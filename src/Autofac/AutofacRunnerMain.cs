using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AddinX.Bootstrap.Contract;
using Autofac;

namespace AddinX.Bootstrap.Autofac
{
    public class AutofacRunnerMain : IRunnerMain
    {
        private readonly CancellationToken token;
        public  ContainerBuilder Builder { get; }

        private IContainer container;

        protected AutofacRunnerMain(CancellationToken token)
        {
            this.token = token;
            Builder = new ContainerBuilder();
        }

        public virtual void Start()
        {
            Task.Factory.StartNew(ExecuteAll, token);
        }

        public virtual void ExecuteAll()
        {
            var runnerInterface = typeof(IRunner);

            var types = Assembly.GetCallingAssembly().GetTypes()
                .Where(p => runnerInterface.IsAssignableFrom(p)
                        && p != runnerInterface);

            foreach (var instance in types.Select(
                    t => (IRunner)Activator.CreateInstance(t)))
            {
                instance.Execute(this);
            }

            container = Builder.Build();
        }

        public IContainer GetContainer()
        {
            return container;
        }
    }
}
