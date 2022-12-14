# Lucene.Net-AWS-Lambda-EFS
Serverless Search for .Net hosted on AWS Lambda with EFStorage.

Use this article for steps on how to connect your lambda function with efs: https://aws.amazon.com/blogs/compute/using-amazon-efs-for-aws-lambda-in-your-serverless-applications/

IMPORTANT: While creating access point for efs, make sure you give 0777 as owner permission. The article tells you to give 777, which caused errors for me.

Lets say that I want "/mnt/lucene" as path to my lucene directory. The path while creating access point should be this path and the mount point of the fileSystem in lambda should also be this path.

### Environment Variables
In visual studio, provide the environment variables in the User Secrets.
In aws lambda, provide the environment variable in the lambda environment variable. (Key: Lucene__Path)
