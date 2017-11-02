namespace Photoblog.Entities {
    public class NewEntryImageDto {

        public byte[] Data { get; set; }

        public string Extension { get; set; }

        public NewEntryImageDto(byte[] data, string extension) {
            Data = data;
            Extension = extension;
        }

    }
}
