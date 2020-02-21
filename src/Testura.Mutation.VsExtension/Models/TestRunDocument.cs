﻿using Prism.Mvvm;
using Testura.Mutation.Core;

namespace Testura.Mutation.VsExtension.Models
{
    public class TestRunDocument : BindableBase
    {
        private MutationDocument _mutationDocument;
        private TestRunStatusEnum _status;
        private string _infoText;

        public enum TestRunStatusEnum
        {
            Running,
            Waiting,
            CompleteAndKilled,
            CompleteAndSurvived,
            CompletedWithUnknownReason
        }

        public MutationDocument Document
        {
            get => _mutationDocument;
            set => SetProperty(ref _mutationDocument, value);
        }

        public TestRunStatusEnum Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string InfoText
        {
            get => _infoText;
            set => SetProperty(ref _infoText, value);
        }
    }
}