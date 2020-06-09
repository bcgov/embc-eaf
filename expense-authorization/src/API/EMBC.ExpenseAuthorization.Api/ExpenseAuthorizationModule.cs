using Autofac;
using AutofacSerilogIntegration;

namespace EMBC.ExpenseAuthorization.Api
{
    public class ExpenseAuthorizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterLogger();
            base.Load(builder);
        }
    }
}
