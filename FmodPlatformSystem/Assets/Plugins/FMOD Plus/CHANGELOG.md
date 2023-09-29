# Changelog
## [1.3.0]-Beta - 2023-09-29
### Fixed
- Doozy 플러그인 서드파티 지원

## [1.2.7]-Beta - 2023-09-29
### Fixed
- FMOD Audio Source에서 IsPlaying() -> isPlaying으로 소문자로 변경
- FMOD Audio Source에서 PlayOnAwake() -> playOnAwake 소문자로 변경

## [1.2.6]-Beta - 2023-09-29
### Fixed
- FMOD Audio Source에서 Mute -> mute으로 소문자로 변경

## [1.2.5]-Beta - 2023-09-29
### Fixed
- FMOD Audio Source에서 Clip -> clip으로 소문자로 변경

## [1.2.4]-Beta - 2023-09-03
### Fixed
- KeyList에서 제공되는 함수 중 TryGetClip()이 추가됨
- TryFind -> TryGet으로 함수 시작부분이 변경됨

## [1.2.3]-Beta - 2023-09-03
### Fixed
- PlayOneShot에서 파라미터 전달 방법 중 ParamRef[] 방식도 추가

## [1.2.2]-Beta - 2023-09-03
### Fixed
- Add(key, path) 함수에서 동일한 이름이 포함된 경우 경고문을 띄우는 알고리즘을 수정함
- 커맨드 센더에서 on API 방식에 대하여 브릿지 템플릿 코드 제공

## [1.2.1]-Beta - 2023-09-03
### Fixed
- 파라미터 센더를 삭제하고 커맨드 센더에 병합 됨
- 트리거 이벤트에 커맨드 센더도 되도록 변경
- FMOD 업데이트 시 내부 코드를 업데이트 하도록 FMOD/FMOD Plus/Force Update를 추가함

## [1.2.0]-Beta - 2023-09-03
### Fixed
- 파라미터 센더를 삭제하고 커맨드 센더에 병합 됨
- 트리거 이벤트에 커맨드 센더도 되도록 변경
- FMOD 업데이트 시 내부 코드를 업데이트 하도록 FMOD/FMOD Plus/Force Update를 추가함

## [1.1.1]-Beta - 2023-09-01
### Fixed
- Audio Listener를 FMOD Studio Listener로 변경하는 메뉴 아이템 추가

## [1.1.0]-Beta - 2023-08-26
### Fixed
- FMOD 2.0.17을 기준으로 업그레이드
- 트리거 이벤트가 파라미터 센더도 되도록 변경

## [1.0.19]-Beta - 2023-08-26
### Fixed
- FMOD Audio Source가 Destroy될 때 오디오가 정지되지 않는 이슈 수정, Audio Source는 객체가 소멸되면 자동으로 오디오가 정지됨
- FMOD 플러그인 2.02.15의 Studio Emitter방식으로 변화됨
- FMODPlus 디파인 추가

## [1.0.18]-Beta - 2023-08-25
### Fixed
- 커맨드 Sender에서 KeyList를 사용할 때 Inital parameter Value에 대한 모호성이 있던 컨셉을 수정

## [1.0.17]-Beta - 2023-08-24
### Fixed
- API Style에서 Key방식을 개편하였습니다.

## [1.0.16]-Beta - 2023-08-24
### Fixed
- 커맨드 Sender에서 Initial Parameter가 재대로 처리안되던 문제 해결

## [1.0.15]-Beta - 2023-08-23
### Fixed
- FMOD Studio Listener 아이콘을 변경하는 로직 변경

## [1.0.14]-Beta - 2023-07-25
### Fixed
- 커맨드 센더 GUI 수정

## [1.0.13]-Beta - 2023-07-25
### Fixed
- 유니티 2021.3 LTS에서도 동작되도록 처리

## [1.0.12]-Beta - 2023-07-25
### Fixed
- MouseDownEvent -> ClickEvent로 변경

## [1.0.11]-Beta - 2023-07-25
### Fixed
- 커멘드 센더 리팩터링 완료 경량화 작업 진행함

## [1.0.10]-Beta - 2023-07-22
### Fixed
- About 추가

## [1.0.9]-Beta - 2023-07-22
### Fixed
- 파라미터 센더 리팩터링 완료 경량화 작업 진행함

## [1.0.8]-Beta - 2023-07-22
### Fixed
- 경로 수정

## [1.0.7]-Beta - 2023-07-22
### Fixed
- Git UPM 지원 중단
- 글로벌 키 리스트 생성 시 해당 폴더가 없으면 에러나는 이슈 수정

## [1.0.6]-Beta - 2023-07-22
### Fixed
- 글로벌/로컬 키 리스트에서 새로운 요소를 만들 때 다른 요소들도 초기화되지 않아있떤 문제 수정

## [1.0.4]-Beta - 2023-07-22
### Fixed
- 글로벌/로컬 키 리스트에서 새로운 요소를 만들 때 Path가 비어있는 값으로 생성되지 않던 문제 수정
