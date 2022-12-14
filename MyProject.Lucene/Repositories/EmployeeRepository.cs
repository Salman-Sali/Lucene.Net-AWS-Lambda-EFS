using Amazon.Lambda.Core;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MyProject.Configrations;
using MyProject.Domain.LuceneEntities;
using Directory = Lucene.Net.Store.Directory;

namespace MyProject.LuceneDb.Repositories
{
    public interface IEmployeeRepository
    {
        public bool AddEmployeeToIndex(Employee employee);
        public List<Employee> Search(string employeeName, bool fuzzySearch);
    }

    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly Directory directory;
        public EmployeeRepository(AppSettings appSettings)
        {
            //install System.Configuration.ConfigurationManager or you will get an error here
            directory = FSDirectory.Open(new System.IO.DirectoryInfo(appSettings.Lucene.Path));
        }

        public bool AddEmployeeToIndex(Employee employee)
        {
            var document = new Document();

            document.Add(new Field(nameof(Employee.EmployeeId), employee.EmployeeId, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field(nameof(Employee.EmployeeName), employee.EmployeeName, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field(nameof(Employee.EmployeeTitle), employee.EmployeeTitle, Field.Store.YES, Field.Index.ANALYZED));

            try
            {
                var idTerm = new Term(nameof(Employee.EmployeeId), employee.EmployeeId);


                using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                using (var writer = new IndexWriter(directory, analyzer, new IndexWriter.MaxFieldLength(1000)))
                {
                    writer.AddDocument(document);

                    writer.Optimize();
                    writer.Flush(true, true, true);

                    return true;
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                using (var writer = new IndexWriter(directory, analyzer, new IndexWriter.MaxFieldLength(1000)))
                {
                    writer.AddDocument(document);
                    writer.Optimize();
                    writer.Flush(true, true, true);

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Employee> Search(string employeeName, bool fuzzySearch = true)
        {
            var result = new List<Employee>();
            using (var reader = IndexReader.Open(directory, true))
            using (var searcher = new IndexSearcher(reader))
            {
                using (Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                {
                    string field = nameof(Employee.EmployeeName);
                    string stringQuery = employeeName;
                    if (fuzzySearch)
                    {
                        stringQuery+="~";
                    }


                    var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, field, analyzer);

                    var query = queryParser.Parse(stringQuery);

                    var collector = TopScoreDocCollector.Create(1000, true);

                    searcher.Search(query, collector);

                    var matches = collector.TopDocs().ScoreDocs;

                    foreach (var item in matches)
                    {
                        var id = item.Doc;

                        var doc = searcher.Doc(id);

                        var resposeItem = new Employee();

                        resposeItem.EmployeeId = doc.GetField(nameof(Employee.EmployeeId)).StringValue;
                        resposeItem.EmployeeName = doc.GetField(nameof(Employee.EmployeeName)).StringValue;
                        resposeItem.EmployeeTitle = doc.GetField(nameof(Employee.EmployeeTitle)).StringValue;

                        result.Add(resposeItem);
                    }
                }
            }
            return result;
        }
    }
}
