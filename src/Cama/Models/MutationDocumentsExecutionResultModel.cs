using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Cama.Core;
using LiveCharts.Defaults;

namespace Cama.Models
{
    public class MutationDocumentsExecutionResultModel : INotifyPropertyChanged
    {
        public MutationDocumentsExecutionResultModel()
        {
            MutationScore = "0%";
            CompletedMutationDocuments = new ObservableCollection<MutationDocumentResult>();
            SurvivedMutationDocuments = new ObservableCollection<MutationDocumentResult>();
            MutationsSurvivedCount = new ObservableValue();
            MutationsKilledCount = new ObservableValue();
        }

#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public ObservableCollection<MutationDocumentResult> CompletedMutationDocuments { get; }

        public ObservableCollection<MutationDocumentResult> SurvivedMutationDocuments { get; }

        public ObservableValue MutationsSurvivedCount { get; }

        public ObservableValue MutationsKilledCount { get; }

        public int FinishedMutationsCount { get; set; }

        public int FailedToCompileMutationsCount { get; set; }

        public string MutationScore { get; set; }

        public void AddResult(MutationDocumentResult result)
        {
            if (!string.IsNullOrEmpty(result.UnexpectedError))
            {
                // Unexpected error so will just ignore it in all statistics
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() => CompletedMutationDocuments.Add(result)));
            FinishedMutationsCount++;

            if (!result.CompilationResult.IsSuccess)
            {
                FailedToCompileMutationsCount++;
                return;
            }

            if (result.Survived)
            {
                MutationsSurvivedCount.Value++;
                Application.Current.Dispatcher.BeginInvoke(new Action(() => SurvivedMutationDocuments.Add(result)));
            }
            else
            {
                MutationsKilledCount.Value++;
            }

            MutationScore = $"{Math.Round((MutationsKilledCount.Value / (MutationsKilledCount.Value + MutationsSurvivedCount.Value)) * 100)}%";
        }

        public void AddResult(IEnumerable<MutationDocumentResult> reportMutations)
        {
            foreach (var mutationDocumentResult in reportMutations)
            {
                AddResult(mutationDocumentResult);
            }
        }
    }
}
