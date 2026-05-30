using Microsoft.ML.Data;

namespace PRACTICA_03.Models
{
    public class SentimientoData
    {
        [LoadColumn(0)]
        public string Texto { get; set; } = string.Empty;

        [LoadColumn(1), ColumnName("Label")]
        public bool Label { get; set; }
    }

    public class SentimientoPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }

        public float Score { get; set; }
    }
}
