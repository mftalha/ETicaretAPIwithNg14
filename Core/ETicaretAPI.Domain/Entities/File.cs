using ETicaretAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Domain.Entities
{
    public class File : BaseEntity
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        [NotMapped] //base entity'de olan UpdatedDate propert'ini ezip File entityde bu column olmıyacakdır: migration'unda olmıyacak yani. == tabi bu alanı base entity 
        public override DateTime UpdatedDate { get => base.UpdatedDate; set => base.UpdatedDate = value; }
    }
}
