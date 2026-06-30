using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Department
{
    public class GetDepartmentDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
