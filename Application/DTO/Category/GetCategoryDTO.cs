using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Category
{
    public class GetCategoryDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
