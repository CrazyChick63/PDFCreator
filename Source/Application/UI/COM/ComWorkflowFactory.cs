﻿using pdfforge.PDFCreator.Core.ComImplementation;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.Core.Workflow.Output;
using SimpleInjector;

namespace pdfforge.PDFCreator.UI.COM
{
    internal class ComWorkflowFactory : IComWorkflowFactory
    {
        private readonly Container _container;

        public ComWorkflowFactory(Container container)
        {
            _container = container;
        }

        public IConversionWorkflow BuildWorkflow(string targetFileName)
        {
            var profileChecker = _container.GetInstance<IProfileChecker>();
            var targetFileNameComposer = new ComTargetFileNameComposer(targetFileName);
            var jobRunner = _container.GetInstance<IJobRunner>();
            var jobDataUpdater = _container.GetInstance<IJobDataUpdater>();
            var outputFileMover = _container.GetInstance<AutosaveOutputFileMover>();

            return new AutoSaveWorkflow(jobDataUpdater, jobRunner, profileChecker, targetFileNameComposer, outputFileMover);
        }
    }
}
