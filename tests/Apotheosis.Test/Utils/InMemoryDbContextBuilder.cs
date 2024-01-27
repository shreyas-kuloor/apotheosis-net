﻿using System.Xml.Linq;
using Apotheosis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Apotheosis.Test.Utils;

public static class InMemoryDbContextBuilder
{
    public static ApotheosisDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApotheosisDbContext>()
            .UseInMemoryDatabase(databaseName: "apotheosis")
            .Options;

        return new ApotheosisDbContext(options);
    }
}
