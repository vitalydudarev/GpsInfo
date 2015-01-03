namespace GpsInfo
{
    public enum GpsTags
    {
        /// <summary>
        /// North or South Latitude
        /// </summary>
        LatitudeRef = 0x01,
        Latitude = 0x02,
        /// <summary>
        /// East or West Longitude
        /// </summary>
        LongitudeRef = 0x03,
        Longitude = 0x04,
        Altitude = 0x06,
        ImgDirectionRef = 0x10,
        ImgDirection = 0x11
    }
}
