using Affiliate.Application.Events.Listeners;
using Affiliate.Application.Events;
using Coravel.Events.Interfaces;
using Coravel;

namespace Affiliate.Application.Startup;

public static class EventsRegistration
{
    public static IServiceProvider SetupEvents(this IServiceProvider services)
    {
        IEventRegistration registration = services.ConfigureEvents();

        // add events and listeners here
        registration
            .Register<UserCreated>()
            .Subscribe<EmailNewUser>();

        return services;
    }
}