# PlatformExample
유니티 횡스크롤 이동 + FMOD 사운드 미들웨어 사용에 대한 데모입니다.

![ex1](https://github.com/bnm000215/FMOD-PlatformExample/blob/master/Document/Preview.PNG)

복숭아를 먹으면 음악의 속도가 빨라지고, 독 복숭아를 먹으면 사운드가 종료됩니다.

![ex2](https://github.com/bnm000215/FMOD-PlatformExample/blob/master/Document/Preview2.PNG)

오케스트라 영역에 들어가면 BGM이 오케스트라 풍으로 바뀌고, 8Bit영역에 들어가면 8비트 음악으로 자연스레 전환됩니다.

![ex3](https://github.com/bnm000215/FMOD-PlatformExample/blob/master/Document/preview3.PNG)

볼륨을 조절할 수 있고, FMOD의 파라미터를 조절하는 Stage를 통해 오케스트라 <--> 8Bit로 전환할 수 있습니다.

Pause를 통해 배경음악을 일시정지 <--> 재생 시킬 수 있습니다.

![ex4](https://github.com/bnm000215/FMOD-PlatformExample/blob/master/Document/preview4.PNG)

`Audio Manager`, `GameManager`, `SaveLoadManager`가 각각 구현되어 있습니다.
이는 게임이 시작시 자동으로 생성되도록 구현되어있고, 이는 `Gameplay Ingredient` 에셋을 통해 매니저를 구성했습니다.


#AudioManager
private :  
SceneManagerOnactiveSceneChanged : 씬이 전환될 때, 해당 씬의 BGM을 세팅합니다.

public :
Change(int index) : 배열로 선언된 bgmClip을 기반으로 파라미터로 들어온 index값을 매치하여 BGM을 변환합니다.

Play() : BGM을 재생합니다.

Stop(FMOD.Studio.STOP_MODE SM) : BGM을 정지합니다. (IMMEDIATE : 바로 정지함, ALLOWFADEOUT : Fade되면서 정지함.)  
주의, ALLOWFADEOUT를 사용시 사운드가 페이드가 완료되어 안들리는 시점이 되었을 때 인스턴스 Total 값이 감소됩니다.

setPause(bool pause) : BGM을 일시정지 <--> 재생합니다.

notifyRelease() : 현재 BGM을 릴리즈 합니다. (인스턴스로 올라간 Total값이 감소됩니다.)

setMasterVolume(float Value) : Value값을 기반하여 Master Volume이 제어됩니다.

setBGMVolume(float Value) : Value값을 기반하여 BGM Volume이 제어됩니다.

setSFXVolume(float Value) : Value값을 기반하여 SFX Volume이 제어됩니다.

PlayOneShot(string path) : path값을 기반하여 효과음을 재생합니다.
