using Amazon;

namespace S3Uploader.Helpers
{
    internal class AWSHelper
    {
        public static RegionEndpoint GetRegionEndpoint(string regionName)
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
                    ErrorHelper.HandleError("AWS region not supported, please check your region name and try again");
                    return null;
            }
        }
    }
}
