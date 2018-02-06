namespace GeoNames.Transcriptors
{
    public class LetterToken
    {
        public string RuText { get; set; }

        public string ForangeText { get; set; }

        public int StartPosition { get; set; }
        public int EndPosition { get; set; }

        public LetterToken PrevToken { get; set; }

        public LetterToken NextToken { get; set; }
    }
}
