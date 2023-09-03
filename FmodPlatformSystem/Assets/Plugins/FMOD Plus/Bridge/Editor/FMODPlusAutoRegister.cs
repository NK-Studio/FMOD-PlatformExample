// using Dummy;
// using FMODPlus;
// using UnityEditor;
// using UnityEditor.Events;
//
// [InitializeOnLoad]
// public class FMODPlusAutoRegister
// {
//     static FMODPlusAutoRegister()
//     {
//         ObjectFactory.componentWasAdded -= HandleComponentAdded;
//         ObjectFactory.componentWasAdded += HandleComponentAdded;
//  
//         EditorApplication.quitting -= OnEditorQuiting;
//         EditorApplication.quitting += OnEditorQuiting;
//     }
//     
//     private static void HandleComponentAdded(UnityEngine.Component obj)
//     {
//         if (obj is ExampleAPIStyleDemo demo)
//         {
//             if (demo.TryGetComponent(out EventCommandSender sender))
//             {
//                 switch (sender.BehaviourStyle)
//                 {
//                     case CommandBehaviourStyle.PlayOnAPI:
//                         UnityEventTools.AddPersistentListener(sender.OnPlaySend, demo.Play);
//                         break;
//                     case CommandBehaviourStyle.StopOnAPI:
//                         UnityEventTools.AddPersistentListener(sender.OnStopSend, demo.Stop);
//                         break;
//                     case CommandBehaviourStyle.ParameterOnAPI:
//                         UnityEventTools.AddPersistentListener(sender.OnParameterSend, demo.ChangeParameter);
//                         break;
//                 }
//             }
//         }
//     }
//  
//     private static void OnEditorQuiting()
//     {
//         ObjectFactory.componentWasAdded -= HandleComponentAdded;
//         EditorApplication.quitting -= OnEditorQuiting;
//     }
// }