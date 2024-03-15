using Amazon.S3;
using Amazon.S3.Model;

var s33Client = new AmazonS3Client();

await using var imputStream = new FileStream("./face.jpg", FileMode.Open, FileAccess.Read);

var putObjectRequest = new PutObjectRequest
{
    BucketName = "evg-aws-course",
    Key = "images/face.jpg",
    ContentType = "image/jpeg",
    InputStream = imputStream
};

await s33Client.PutObjectAsync(putObjectRequest);