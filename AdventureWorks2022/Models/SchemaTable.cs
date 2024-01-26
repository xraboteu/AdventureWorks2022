using System.ComponentModel.DataAnnotations;

namespace AdventureWorks2022.Models
{
    public class SchemaTable
    {
        [Key]
        public Guid Id { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string SchemaName { get; set; }
    }
}
