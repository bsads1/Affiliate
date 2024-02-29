using NanoidDotNet;

namespace Affiliate.Application.Extensions
{
    public static class GuidExtension
    {
        public static string TaoUid(int size = 10, bool haveNumber = true, bool haveUpperCase = true,
            bool haveLowerCase = false)
        {
            var allowChar = "";
            if (haveNumber)
            {
                allowChar += "0123456789";
            }

            if (haveLowerCase)
            {
                allowChar += "qwertyuiopasdfghjklzxcvbnm";
            }

            if (haveUpperCase)
            {
                allowChar += "QWERTYUIOPASDFGHJKLZXCVBNM";
            }

            return Nanoid.Generate(allowChar, size);
        }

        public static Guid TaoGuid()
        {
            var guid = Guid.NewGuid();
            return guid;
        }

        public static string ToStringUpper(this Guid guid)
        {
            return guid.ToString().ToUpper();
        }

        public static Guid ToGuid(this string guidString)
        {
            if (Guid.TryParse(guidString, out var guid))
            {
                return guid;
            }

            return Guid.Empty;
        }
    }
}