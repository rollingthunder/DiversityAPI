using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DiversityService.API.Services
{
    public static class MappingExtensions
    {
        public static string GetSHA1Hash(this DB.TaxonNames.TaxonList list)
        {
            if(list == null)
            {
                return string.Empty;
            }

            var id_string = string.Format("{0}_{1}_{2}_{3}",
                list.ListID,
                list.DataSource,
                list.DisplayText,
                list.TaxonomicGroup);
            var id_bytes = Encoding.UTF8.GetBytes(id_string);

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(id_bytes);
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}