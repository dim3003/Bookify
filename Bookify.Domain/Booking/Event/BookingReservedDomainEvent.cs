using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Booking.Event;

public sealed record BookingReservedDomainEvent(Guid BookingId) : IDomainEvent;
