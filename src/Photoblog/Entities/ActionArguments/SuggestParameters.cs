namespace Photoblog.Entities.ActionArguments {
    public class SuggestParameters {

        public double? Lat { get; set; }

        public double? Lon { get; set; }

        public bool ContainsLocation => Lat != null && Lon != null;

    }
}
