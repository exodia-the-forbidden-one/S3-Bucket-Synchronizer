using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Linq;

namespace Uploader_UI.Helpers;

public class AwsHelper
{
    public async Task<bool> IsCredentialsValidAsync(string accessKey, string secretKey, string region)
    {
        try
        {
            using (var client = new AmazonS3Client(accessKey, secretKey, GetRegionEndpoint(region)))
            {
                await client.ListBucketsAsync();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> GetBuckets(string accessKey, string secretKey, string region)
    {
        try
        {
            RegionEndpoint? regionEndpoint = GetRegionEndpoint(region);
            using (var client = new AmazonS3Client(accessKey, secretKey, regionEndpoint))
            {
                ListBucketsResponse response = await client.ListBucketsAsync();
                return response.Buckets.Select(b => b.BucketName).ToList();
            }
        }
        catch
        {
            return new List<string>();
        }
    }

    public static RegionEndpoint? GetRegionEndpoint(string regionName)
    {
        switch (regionName)
        {
            case "EUWest1":
                return RegionEndpoint.EUWest1;
            case "EUWest2":
                return RegionEndpoint.EUWest2;
            case "EUWest3":
                return RegionEndpoint.EUWest3;
            case "EUCentral1":
                return RegionEndpoint.EUCentral1;
            case "EUCentral2":
                return RegionEndpoint.EUCentral2;
            case "EUNorth1":
                return RegionEndpoint.EUNorth1;
            case "EUSouth1":
                return RegionEndpoint.EUSouth1;
            case "EUSouth2":
                return RegionEndpoint.EUSouth2;
            case "USEast1":
                return RegionEndpoint.USEast1;
            case "USEast2":
                return RegionEndpoint.USEast2;
            case "USWest1":
                return RegionEndpoint.USWest1;
            case "USWest2":
                return RegionEndpoint.USWest2;
            case "MECentral1":
                return RegionEndpoint.MECentral1;
            case "MESouth1":
                return RegionEndpoint.MESouth1;
            case "AFSouth1":
                return RegionEndpoint.AFSouth1;
            case "APSouth2":
                return RegionEndpoint.APSouth2;
            default:
                return null;
        }
    }
}