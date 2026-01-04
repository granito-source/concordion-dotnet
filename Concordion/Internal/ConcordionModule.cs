using Ninject.Modules;

namespace Concordion.Internal;

class ConcordionModule : NinjectModule
{
    public override void Load()
    {
        //Bind<IExecuteStrategy>().ToProvider();
    }
}