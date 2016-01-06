using UnityEngine;
using System.Collections;

public class RandomPlacer : PipeItemGenerator {

    public PipeItem[] itemPrefabs;

    public override void GenerateItems(Pipe pipe)
    {
        int countOfTurnTable = 0;
        for (int i = 0; i < pipe.CurveSegmentCount; i+= 2)
        {
            int pointer = Random.Range(0, itemPrefabs.Length);
            PipeItem item = Instantiate<PipeItem>(
            itemPrefabs[pointer]);
            //保证Turntabl只出现一次
            if(countOfTurnTable == 0)
            {
                if(item.GetType() == typeof(PipeItemTurntable))
                    countOfTurnTable++;
            }else if(item.GetType() == typeof(PipeItemTurntable)){
                pointer = (pointer+1)%itemPrefabs.Length;
                item = Instantiate<PipeItem>(
                        itemPrefabs[pointer]);
            }

            float pipeRotation =
                (Random.Range(0, pipe.pipeSegmentCount) + 0.5f) *
                360f / pipe.pipeSegmentCount;
            item.Position(pipe, i, pipeRotation, pipe.GetPipeRadiusBySegmentIndex(i));
        }
    }
}
