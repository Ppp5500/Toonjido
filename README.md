0.0.6
CameraWork 추가
	- 터치로 화면 이동
	- 줌인, 줌아웃

0.0.7
City 모델링
	- GPS 이동 거리와 비율을 맞추기 위해 스케일을 0.265로 조정

GPSEncoder
	- 원점을 천안역 앞으로 설정

CameraWork
	- 아이레벨 회전 기능
	- 조작상태에 따라 조작 방식 달라지도록 적용

JoyStickMove
	- UIVirtual 조이스틱의 값을 받아서 움직이는 스크립트

MainCanvas
	- SideSearch 추가

SafeArea
	- 노치 대응 위한 SafeArea 적용

CurrentControl
	- 현재 조작 상태를 저장
	- 조작상태 변경 시 호출 될 EventHandler

무료 에셋인 cell shader 적용

0.0.10
CameraWork
	- 조이스틱과 화면 회전 버그 수정
	- 줌인/아웃 방식을 카메라의 position을 변경하는 것에서 field of view를 변경하는 방식으로 변경
	- ResetPosToPlayer 추가
	- GetEncounter Raycast로 오브젝트 감지
SearchResult
	- 검색 결과를 보여줄 클래스

0.1.2
appSetting에 baseURL 설정
SearchResult -> SearchManager로 바꿈
	- http 통신을 통해서 백엔드에서 검색 결과 가져옴
WeatherManger
	- http 통신을 통해서 벡엔드에서 날씨 결과 가져옴
Scripts/StructuralClasses
	- User 클래스 추가
	- Weather 클래스 추가
	- SearchedStore 클래스 추가
Scripts/StructuralClasses/Util
	- PlayerDataSaver 추가
		- 사용자 정보, 토큰 저장
		- 토큰 암호화
Scripts/Login
	- DeepLinkManager
		- 딥링크 활성화 및 딥링크 메시지 처리
	- LoginManger
		- 카카오 로그인 링크 열어줌
안드로이드/iOS 딥링크 설정함 appdev://

0.1.6
텍스쳐 오류 해결
흥룡, 칠성구 추가
shootball 추가
cinemachine 추가

0.2.0
URP로 변경
Lens Flare 변경
Global Volume 생성 및 포스트 프로세싱 설정
건물들 셰이더 URP/Lit으로 변경

0.2.1
텍스쳐 오류 진짜 해결
Script/MoveSet/CameraControl
	- 아이레벨에서 두 손가락 터치 시 화면이 의도하지 않은 곳으로 돌아가는 현상 수정
	- 아이레벨에서 건물 터치 시, 터치 한 곳으로 오브젝트를 이동시기는 기능 추가

DepthFilter 추가(못 생겨서 쓰진 못 할듯)

0.2.6
UI 1차 시안

0.4.5
텍스처 오류는 오브젝트를 static으로 설정하면 나타나는 현상이엇음....
navmesh를 이용하여 길찾기 구현

0.6.1
디자인 변경 - 로딩화면, 메인 화면 구성과 아이콘, 날씨화면, 버스 화면 추가, 카테고리 토글 변경, 검색 결과 창 디자인 적용
기타 등등 매우 많음
Localization 패키지 설치 및 어플 이름 한국어 적용

1.0.1
앱 아이콘 적용, 메뉴화면, 설정 화면 추가
버스 화면 수정
Vr Manager 및 Vr Objects 추가
모델링 23년 06월 08일 것으로 변경
localization table 수정
	- en-US 제거
	- en 추가
아이폰 구동 확인

1.0.2
simple toon shader 적용
overlook 모델링들의 머테리얼 색 변경

1.0.3
검색 목록에서 별 갯수가 평점을 반영하도록 구현
찜 목록 삭제 기능 추가
실수로 await 키워드 빼먹은 메소드들에 await 추가

1.0.4
메뉴 화면에 앱 종료, 로그아웃 버튼 추가
Notice manager 수정
앱 시작 시 자동으로 주변 검색 기능 실행

1.0.5
UIdrag에 half 메소드 추가
section 선택 시 모달을 half로 표시, 섹션을 화면의 2/3지점에 둠
iOS status bar 표시
SearchManager 최적화

1.1.4
WeatherCanvas 변경
AppleManager 추가
	- FaceID를 이용하여 Apple서버에 계정 연동 요청
Webview 추가
	- 카카오로그인 시 브라우저가 열리는 것이 아닌 Webview를 통해서 로그인하도록 변경

1.1.5
AppleManager에서 AppleLogin 시 API서버에 회원가입 요청하도록 추가
InitManager
AccountChecker

1.1.6
날씨 UI에 데이터 연동
로그인 시스템 별로 로그아웃 기능 설정
로그인 시스템 별로 회원탈퇴 기능 설정
iphone에서 Playerprefs를 이용한 데이터 저장이 안 되길래 그냥 PlayerInfoSaver로 저장시켜버림
흐린 날씨일때 reflection cude 변경

1.2.0_03
오브젝트들에 Oclude culling 설정
시장 내 천장의 일부분을 투명하게 변경
시장 내 바닥의 색을 조금 더 초록색으로 변경
일부 머테리얼의 셰이더 변경
툰셰이더를 사용한 머테리얼의 rim 값 변경

1.2.1
Section을 표시 할 때 카테고리에 해당하는 이미지도 표시하도록 추가
Section을 표시 할 때 폰트 색을 카테고리에 맞춰서 변경하도록 추가
Android에서 BackKey 대응
	- BackKeyManager 수정
	- CanvasListItem 추가
	- CanvasListItemPrent 추가

1.2.1_02
CanvasListItem 활성화 시에 애니메이션 추가
SignCanvases에 sign 추가
Marble Targets
Rain_heavy에 Parent Constraint 추가하여 항상 같은 각도를 유지하도록 변경
CameraWork
	- holdSlider가 건물 앞에 있는 버튼을 눌렀을 때도 나타나는 현상 수정
	- Update()에 State.Weather 상태일 때의 로직 추가, 제자리에서 화면을 돌릴 수 있도록 추가
WeatherManager 
	- DisplayWeather 수정 overlook에서 진입 할 때와 eyelevel에서 진입할 때 각각 다른 화면을 보여주도록 변경
	- ResetWather 추가 다른 상태로 변환 시 흥룡이가 안 보이도록 처리
SevenStarMarbleGameManager 추가
	- 하루 중 최초 접속 시 흥룡이가 등장하는 컷신 보여주도록 추가
	- 흥룡이가 이미 지정된 위치에 칠성구를 뿌리도록 추가

1.2.2
SevenStarMarbleGameManager 수정
MarbleToCollect 추가
	- 칠성구 획득
	- 각 Ballprefab에 추가
FortuneWindowManager 추가
	- csv 파일을 캐싱
	- ShowFortune() 호출 시 랜덤으로 운세 보여줌
FortuneFileReader
	- csv 파일 읽어오는 기능

1.3.1
SevenStarMarbleGameManager 수정
	- 이벤트 발생 시 저장된 정보 수정
	- 이벤트 발생 시 창 기능(새로운 디자인) 호출

PlayerInfoSaver 수정
	- 흥룡이가 뿌린 칠성구 위치를 저장
	- 저장된 칠성구 위치 불러오기

1.3.3
Webviewer
	- 웹뷰를 띄울 때 자동으로 캐시와 쿠키를 삭제하도록 설정
날씨화면
	- 미세먼지 패널 색이 미세먼지 상황에 따라서 변하도록 기능 추가
	- 현재 기상 상황을 반영한 아이콘을 표시하도록 추가
	- 흥룡이가 한 자리에 떠있는 것이 아닌 맵 전체를 돌아다니도록 변경
길찾기 기능
	- 건물 내부에서 길 찾기 기능 사용 시 현재 위치가 떨리는 현상 수정
	- 길찾기 도착 아이콘 변경
	- 길찾기 중지 버튼 추가
	- 길찾기 시 차도를 건너지 않도록 변경
	- 명지역길 외부에서 길찾기 기능 사용 시 카카오맵 열리도록 설정
메인 화면
	- ButtonMakerVer2 추가
		- 서버에서 가게 위치 데이터를 불러와 지도 상에 표시
		- 카테고리 별로 아이콘 바뀌도록 추가
		- 클릭 시 해당 가게 상세정보 보여주도록 추가
	- 
가게 상세정보화면
	- 길찾기 버튼 크게 변경

1.3.5
길찾기 기능
	- 가로등 같은 작은 오브젝트들 navobject로 사용되지 않게 속성 변경
	- 1초 마다 경로 재탐색
	- 경로 텍스처 변경
	- 길찾기 시 출발, 도착 위치 표시
날씨 화면
	- 미세먼지, 습도 패널에 아이콘 추가
1.3.6
프로필 설정 기능
	- 프로필 설정 UI 제작(NicknameSettingCanvas)
	- NiicknameManager 작성
시장 바닥 디자인 변경

1.4.0
프로필 설정
	- 비로그인 상태에서는 프로필 설정창이 안 열리도록 설정
SettingCanvas
	- 최대 이동속도를 더 높게 설정
GPS Modifing Manager
	-  GPS가 건물 안으로 잡힐 경우 근처 도로를 탐색해서 위치를 보정
NavPlayerControl
	- 1인칭에서 길찾기 시 시작지점을 쳐다보도록 설정
API target을 33으로 변경

1.4.0_02
프로필 설정
	- 최초 프로필 설정 시 서버에 먼저 프로필 정보를 초기화하도록 설정

1.4.1
코드 전체에서 ball과 marble 혼용하던 걸 marble로 통일
각 씬 로딩 후 로딩 화면 Setactive false, 03 TestScene 로딩 후에는 더 이상 사용하지 않으므로 Destory

1.4.2
1인칭에서 GPS 버튼 누르면 엉뚱한 위치로 이동되는 현상 -> 해당 버튼이 CameraWork의 ResetPOs method를 호출하는데 해당 method는 Modified GPS Coor가 아닌 Actucal GPS Coor위치로 이동하도록 되어 있었음, Modified GPS Coor위치로 이동하도록 변경
SetDestination 실행시 첫 경로를 천안역 앞으로 하는 버그가 있음(주로 Section10 쪽에서 발생) -> 천안역 근처 위치와 Section10 쪽 Blocker 설치하여 길을 막음 일단 지금 당장은 해당 현상 일어나지 않음
기존에 가게 정보를 받아 올때 연락처에 전화기 특수문자가 포함되어 있었음 이를 단순히 문자열 앞의 두글자를 자르는 방식으로 해결했었는데 이를 정규식을 사용하도록 변경(SearchManager)
SignCanvases의 상점을 나타내는 아이콘들에 Button Component 추가하고 누르면 SearchManager의 OpenDetatil 실행하도록 설정
일정 시간 마다가 아니라 하루에 첫 접속 시에 구슬을 날리도록 설정(CheckDragonEventRecordForToday 작성, DragonEventAsyncForToday 작성)

1.4.2_04
NavMeshAgent 대신에 Naver 길찾기 API 사용하도록 변경
	- NaverDirectionManager 작성
	- 경로를 그리는 PathDrawer 작성
MainUICanvas ver2
	- 날씨 아이콘 애니메이션 제거하고 이미지 변경
	- 1인칭 아이콘 애니메이션 변경

1.4.2_07
네이버 길찾기를 사용할 때, 유저가 목표지점에 도착했음을 파악하는 기능 제작
	- PlayerArrivalCheck 작성

1.4.4
접속 시에 서버와의 연결을 확인하고 실패하면 종료하는 안내창 표시
	- Init Manager 수정
리뷰 창 제작 및 스크립트 작성
	- ReviewManager  작성
	- SeartchManager 수정

1.4.06
오류 수정