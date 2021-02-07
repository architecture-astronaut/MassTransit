namespace MassTransit.Containers.Tests.FutureScenarios.Futures
{
    using System.Linq;
    using Automatonymous;
    using Contracts;
    using MassTransit.Futures;
    using MassTransit.Futures.Internals;


    public class CalculateFuture :
        Future<OrderCalculate, CalculateCompleted>
    {
        public CalculateFuture()
        {
            Event(() => CommandReceived, x => x.CorrelateById(context => context.Message.OrderLineId));

            FutureResponseHandle<OrderCalculate, CalculateCompleted, Fault<OrderCalculate>, OrderFry, FryCompleted> fry = SendRequest<OrderFry>(x =>
                {
                    x.UsingRequestInitializer(context => new
                    {
                        OrderId = context.Instance.CorrelationId,
                        OrderLineId = InVar.Id,
                        Size = Size.Medium
                    });

                    x.TrackPendingRequest(message => message.OrderLineId);
                })
                .OnResponseReceived<FryCompleted>(x =>
                {
                    x.CompletePendingRequest(message => message.OrderLineId);

                    x.WhenReceived(binder => binder.Then(context =>
                    {
                        FutureConsumeContext<FryCompleted> consumeContext = context.CreateFutureConsumeContext();

                        consumeContext.SetVariable("Taste", "Delicious");
                    }));
                });


            WhenAllCompleted(x =>
            {
                x.SetCompletedUsingInitializer(context =>
                {
                    var fryCompleted = context.SelectResults<FryCompleted>().FirstOrDefault();

                    context.TryGetVariable("Taste", out string taste);

                    return new {Description = $"Calculated ({fryCompleted.Description}, {taste})"};
                });
            });
        }
    }
}
