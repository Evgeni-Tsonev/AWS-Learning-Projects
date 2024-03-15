using Amazon.S3;
using Amazon.S3.Model;
using System.Text;

var s33Client = new AmazonS3Client();

var getObjectRequest = new GetObjectRequest
{
    BucketName = "evg-aws-course",
    Key = "files/movies.csv",
};

var response = await s33Client.GetObjectAsync(getObjectRequest);

using var ms = new MemoryStream();
response.ResponseStream.CopyTo(ms);

var text = Encoding.Default.GetString(ms.ToArray());

Console.WriteLine(text);


//await using var imputStream = new FileStream("./face.jpg", FileMode.Open, FileAccess.Read);

//var putObjectRequest = new PutObjectRequest
//{
//    BucketName = "evg-aws-course",
//    Key = "files/movies.csv",
//    ContentType = "text/csv",
//    InputStream = imputStream
//};

//await s33Client.PutObjectAsync(putObjectRequest);