using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Angsthaas.Models;

namespace Angsthaas
{
    internal class Program
    {
        private static void Main()
        {
            // Set application culture.
            SetApplicationCulture();

            // Start bot logic. (blocking until done)
            Bot.Start(Strategy);
        }

        public static Move[] Strategy(GameState gamestate)
        {
            var moves = new List<Move>();

            var myPlanets = gamestate.Planets.Where(p =>
                p.Owner == gamestate.Settings.PlayerId &&
                p.Health >= 0.5 * p.Radius
            );

            foreach (var planet in myPlanets)
            {
                var targets = planet.Neighbors
                    .Select(n => gamestate.Planets[n])
                    .Where(p => p.Owner != gamestate.Settings.PlayerId)
                    .OrderByDescending(p =>
                        gamestate.Planets
                            .Where(e => e.Owner.HasValue && e.Owner != gamestate.Settings.PlayerId)
                            .Select(e => p.DistanceTo(e))
                            .Max()
                    )
                    .Take(2);

                foreach (var target in targets)
                {
                    var power = gamestate.Planets[planet.Id].Health * 4 / 9;
                    moves.Add(new Move(power, planet.Id, target.Id));
                }
            }

            return moves.ToArray();
        }

        private static void SetApplicationCulture()
        {
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}