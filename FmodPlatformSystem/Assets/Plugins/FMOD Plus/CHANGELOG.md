# Changelog
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
