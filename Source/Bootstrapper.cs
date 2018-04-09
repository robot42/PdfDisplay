// <copyright>
//     Copyright (c) AIS Automation Dresden GmbH. All rights reserved.
// </copyright>

namespace PdfDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;

    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer container = new SimpleContainer();

        public Bootstrapper()
        {
            this.Initialize();
        }

        protected override void Configure()
        {
            var repository = new RecentFilesRepository();

            this.container.Singleton<IEventAggregator, EventAggregator>();
            this.container.Singleton<IWindowManager, WindowManager>();
            this.container.RegisterInstance(typeof(IRecentFilesQuery), string.Empty, repository);
            this.container.RegisterInstance(typeof(RecentFilesRepository), string.Empty, repository);
            this.container.Singleton<WelcomeViewModel, WelcomeViewModel>();
            this.container.Singleton<DocumentViewModel, DocumentViewModel>();
            this.container.Singleton<DocumentNotFoundViewModel, DocumentNotFoundViewModel>();
            this.container.Singleton<DocumentHistoryViewModel, DocumentHistoryViewModel>();
            this.container.Singleton<MainViewModel, MainViewModel>();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            return this.container.GetInstance(serviceType, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return this.container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            this.container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            this.DisplayRootViewFor<MainViewModel>();
        }
    }
}