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
            this.container.Singleton<IEventAggregator, EventAggregator>();
            this.container.Singleton<IWindowManager, WindowManager>();
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