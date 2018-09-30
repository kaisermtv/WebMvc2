using System;
using WebMvc.Application.Lib;

namespace WebMvc.Application.Entities
{
    public partial class PollAnswer
    {
        public PollAnswer()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Answer { get; set; }
    }
}
