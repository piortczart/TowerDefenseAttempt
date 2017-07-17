using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TowerDefenseColab.GraphicsPoo.SpriteUnicorn;

namespace TowerDefenseColab.GamePhases.GameLevels.LevelLayouts
{
    public class LevelLayoutLoader
    {
        public SpriteEnum[,] LoadLevelLayout(string levelId)
        {
            string json = File.ReadAllText("GamePhases/GameLevels/LevelLayouts/" + levelId + ".json");
            SpriteEnum[][] jagged = JArray.Parse(json).Select(a => a.Select(b => (SpriteEnum)(int)b).ToArray()).ToArray();
            return JaggedToMultidimensional(jagged);
        }

        private TArray[,] JaggedToMultidimensional<TArray>(TArray[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray.Max(subArray => subArray.Length);
            TArray[,] array = new TArray[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                cols = jaggedArray[i].Length;
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = jaggedArray[i][j];
                }
            }
            return array;
        }
    }
}