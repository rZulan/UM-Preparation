using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Pond
{
    public class GetPondDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
