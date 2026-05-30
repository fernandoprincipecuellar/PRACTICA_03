using System;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using PRACTICA_03.Models;

namespace PRACTICA_03.Services
{
    public class SentimientoService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly object _lock = new object();

        public SentimientoService()
        {
            _mlContext = new MLContext(seed: 0);
            var dataPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "sentimientos.csv");
            var data = _mlContext.Data.LoadFromTextFile<SentimientoData>(dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = _mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimientoData.Texto))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SentimientoData.Label), featureColumnName: "Features"));

            _model = pipeline.Fit(data);
        }

        public string Predecir(string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario)) return "Negativo";

            var predEngine = _mlContext.Model.CreatePredictionEngine<SentimientoData, SentimientoPrediction>(_model);
            var input = new SentimientoData { Texto = comentario };
            var pred = predEngine.Predict(input);
            return pred.PredictedLabel ? "Positivo" : "Negativo";
        }
    }
}
