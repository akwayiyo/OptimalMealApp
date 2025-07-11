using Microsoft.AspNetCore.Mvc;

namespace OptimalMealApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptimizeController : ControllerBase
    {
        public class IngredientInput
        {
            public Dictionary<string, int> Ingredients { get; set; }
        }

        [HttpPost]
        public IActionResult Optimize([FromBody] IngredientInput input)
        {
            var recipes = new List<(string Name, int Feeds, Dictionary<string, int> Ingredients)> {
                ("Burger", 1, new() { {"Meat", 1}, {"Lettuce", 1}, {"Tomato", 1}, {"Cheese", 1}, {"Dough", 1} }),
                ("Pie", 1, new() { {"Dough", 2}, {"Meat", 2} }),
                ("Sandwich", 1, new() { {"Dough", 1}, {"Cucumber", 1} }),
                ("Pasta", 2, new() { {"Dough", 2}, {"Tomato", 1}, {"Cheese", 2}, {"Meat", 1} }),
                ("Salad", 3, new() { {"Lettuce", 2}, {"Tomato", 2}, {"Cucumber", 1}, {"Cheese", 2}, {"Olives", 1} }),
                ("Pizza", 4, new() { {"Dough", 3}, {"Tomato", 2}, {"Cheese", 3}, {"Olives", 1} })
            };

            var bestFeed = 0;
            Dictionary<string, int> bestCombo = null;

            void TryCombo(int[] combo)
            {
                var totalUsed = new Dictionary<string, int>();
                var feed = 0;

                for (int i = 0; i < combo.Length; i++)
                {
                    foreach (var ing in recipes[i].Ingredients)
                    {
                        if (!totalUsed.ContainsKey(ing.Key))
                            totalUsed[ing.Key] = 0;
                        totalUsed[ing.Key] += ing.Value * combo[i];
                    }
                    feed += recipes[i].Feeds * combo[i];
                }

                if (totalUsed.All(x => input.Ingredients.ContainsKey(x.Key) && input.Ingredients[x.Key] >= x.Value))
                {
                    if (feed > bestFeed)
                    {
                        bestFeed = feed;
                        bestCombo = recipes.Select((r, i) => new { r.Name, Count = combo[i] })
                                           .Where(x => x.Count > 0)
                                           .ToDictionary(x => x.Name, x => x.Count);
                    }
                }
            }

            void GenerateCombos(int[] combo, int index)
            {
                if (index == combo.Length)
                {
                    TryCombo(combo);
                    return;
                }
                for (int i = 0; i <= 5; i++)
                {
                    combo[index] = i;
                    GenerateCombos(combo, index + 1);
                }
            }

            GenerateCombos(new int[recipes.Count], 0);

            return Ok(new { TotalPeopleFed = bestFeed, RecipeCounts = bestCombo });
        }
    }
}
