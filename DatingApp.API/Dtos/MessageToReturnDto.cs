using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Dtos
{
    public class MessageToReturnDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderKnownAs { get; set; }
        public int RecieverId { get; set; }
        public string RecieverKnownAs { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        public string SenderPhotoUrl { get; set; }
        public string RecieverPhotoUrl { get; set; }
    }
}
