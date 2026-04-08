using System;
using System.Collections.Generic;
using System.Text;

namespace GruersShop.Services.Common.Exceptions;

public class ExceptionMessages
    {
        public const string RepoInterfaceNotFound = "The {0} could not be added to Service Collection, because no interface matching the convention could be found! " +
            "Convention : I<ClassName>.";
    }
