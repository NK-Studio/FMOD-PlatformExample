using BehaviorDesigner.Runtime.Tasks;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
    [TaskCategory("FMODPlus")]
    [TaskIcon("Assets/Gizmos/FMODPlus/FMODStop.png")]
    [TaskDescription("사운드를 정지합니다.")]
    public class FMODStop : Action
    {
        public bool Fade;
        public SharedFMODAudioSource FMODAudioSource;

        public override TaskStatus OnUpdate()
        {
            if (FMODAudioSource.Value)
            {
                FMODAudioSource.Value.Stop(Fade);
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}