namespace GeoNames.Transcriptors
{
  public  interface ITranscriptor
  {
      string ToRussian(string text);
      string Language { get; }
      string TableInBase { get; set; }
      int Id { get; }
  }
}
