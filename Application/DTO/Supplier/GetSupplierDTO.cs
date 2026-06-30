using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.Supplier
{
    public class GetSupplierDTO
    {
        public required int Id { get; set; }
        public required bool IsActive { get; set; }
        public required string Name { get; set; }
    }
}
