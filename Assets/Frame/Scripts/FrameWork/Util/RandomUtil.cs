using System;
using System.Collections;
using System.Collections.Generic;

public class RandomBody
{
    public RandomBody() { }
    public RandomBody(int index, int weight)
    {
        Weight = weight;
        Index = index;
    }
    public int Weight;
    public int Index;
}

public class RandomUtil
{


    public static int GetRandomIndexByWeight(List<RandomBody> randomBodyList)
    {
        //计算权重总和
        int totalWeights = 0;
        for (int i = 0; i < randomBodyList.Count; i++)
        {
            totalWeights += randomBodyList[i].Weight;
        }

        List<int> numList = new();
        foreach (RandomBody randomBody in randomBodyList)
        {
            int weight = randomBody.Weight;
            for (int i = 0; i < weight; i++)
            {
                numList.Add(randomBody.Index);
            }
        }
        int randomPos = new Random().Next(totalWeights);
        var list = RandomSort(numList);
        int number = 0;
        if (list.Count > randomPos)
        {
            number = list[randomPos];
        }
        return number;
    }

    public static List<T> RandomSort<T>(List<T> list)
    {
        var random = new Random();
        var newList = new List<T>();
        foreach (var item in list)
        {
            newList.Insert(random.Next(newList.Count), item);
        }
        return newList;
    }

    /// <summary>
    /// 随机获取一个 list 中的值
    /// </summary>
    public static T GetRandomT<T>(List<T> list)
    {
        var random = new System.Random();
        int index = random.Next(0, list.Count);
        return list[index];
    }

    public static int RandomRange(int min, int max)
    {
        // 左闭右开
        return new System.Random().Next(min, max);
    }
    public static int GetRandom(int max)
    {
        // 左闭右开
        return new System.Random().Next(max);
    }

    public static double NextDouble(double minValue, double maxValue)
    {
        return new Random().NextDouble() * (maxValue - minValue) + minValue;
    }

    public static void ShuffList<T>(List<T> sources)
    {
        Random rd = new Random();
        int index = 0;
        T temp;
        for (int i = 0; i < sources.Count; i++)
        {
            index = rd.Next(0, sources.Count - 1);
            if (index != i)
            {
                temp = sources[i];
                sources[i] = sources[index];
                sources[index] = temp;
            }
        }
    }
}
