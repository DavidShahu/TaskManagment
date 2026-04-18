using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Primitives
{
    public interface IDomainEvent
    {

        //keeeping track of events so it can be tracked
        public interface IDomainEvent
        {
            Guid Id { get; }
            DateTime OccurredOn { get; }
        }
    }
}
