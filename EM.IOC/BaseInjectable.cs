namespace EM.IOC
{
    [Injectable(ServiceLifetime = ServiceLifetime.Scoped)]
    public abstract class BaseInjectable : IInjectable
    {
    }
}