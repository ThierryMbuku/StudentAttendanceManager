using SAM1.CrossCuttingConcerns.ResponseModels;
using System;
using System.Collections.Generic;

namespace SAM1.Models
{
    public class SigninProgressResponseModel : ResponseModel
    {
        private List<Result> progress;

        internal SigninProgressResponseModel()
        {
            progress = new List<Result>();
        }

        internal void CreateProgressItem(string seriesTitle, float seriesValue)
        {
            progress.Add(new Result
            {
                SeriesTitle = seriesTitle,
                SeriesValue = Math.Round(seriesValue, 2)
            });
        }

        internal List<Result> GetProgressList()
        {
            return progress;
        }
    }

    public class Result
    {
        public string SeriesTitle { get; set; }
        public double SeriesValue { get; set; }
    }
}