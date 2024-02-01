using System.ComponentModel.DataAnnotations;

namespace ChunkUpload.Model
{
    [Serializable]
    public class CreateSessionParams
    {
        [Required]
        public int? ChunkSize { get; set; }
        [Required]
        public long? TotalSize { get; set; }
        [Required]
        public string FileName { get; set; }
    }
}