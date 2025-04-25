# Project Solvive
~~파티 애니멀즈의 서로를 밀어내어 안전구역을 이탈 시키는 게임성 + Totally accurate battlegrounds의 랙돌 캐릭터 감성을 접목한 멀티플레이 FPS 게임~~<br>
1인칭 마피아 탈출 생존 게임
<br><br><br>

## 개발 기간
2024.09.30 ~ 진행 중
<br><br><br>

## 팀 구성원
- 팀장 : 최대영
- 팀원 : 이성훈, 이주환
<br><br><br>


## 시나리오
기억을 잃은 인간들이 마네킹 공장에서 눈을 뜬다.<br>
마네킹으로 변해 버린 인간들은 열쇠를 획득하여 공장에서 탈출을 시도한다.<br>
마네킹은 인간들의 영혼을 흡수하여 진짜 인간이 될 수 있다.<br>
인간들의 탈출이 빠를 것인가, 마네킹의 학살이 빠를 것인가
<br><br><br>

## 게임 플레이
~~- 랙돌 피직스 캐릭터가 총기를 주운 후 사격~~
~~- 피격 되는 캐릭터는 총기 종류에 따라 일정 거리 넉백~~
~~- 넉백으로 인한 안전 구역 이탈 시 사망 처리~~
~~- 팀 또는 개인 전원이 안전 구역 이탈 시 패배~~
~~- 필드 드랍~~
~~- 단도 : 조준점 크기만큼의 찌르기 공격~~
~~- 권총 : 2발의 총알을 사격하여 공격~~
- 피격 시스템
  - 칼
    - 머리 : 100
    - 몸통 : 60
    - 팔다리 : 20
  - 총
    - 머리 : 100
    - 몸통 : 60
    - 팔다리 : 30
- 탈출구
  - 탈출구의 내외부에 버튼이 존재
  - 열쇠 소유자가 버튼을 누르고 있는 동안에만 탈출구가 활성화
- 인간
  - 2인이 협동 하여 미션을 성공하고 카드키를 획득하여 공장을 탈출
  - 마네킹을 식별하여 사살
- 마네킹
  - 모든 인간을 사살
  - 공장을 탈출 시 공장의 모든 전력이 다운되고 강력한 마네킹으로 환생
<br><br><br>

## 파밍 요소
- 권총 : 2발의 탄약 
- 칼
- 배터리 : 탈출 미션을 위해 필요
<br><br><br>

## 최대영 개발 내용
- Jira + Git + Notion을 통한 Unity 팀 개발 환경 구축
- GameScene - 플레이어 캐릭터 제외 모든 게임 로직 개발
- GameScene - UI, UX 기획 및 구현
<br><br><br>

## 이주환 개발 내용
- MainScene - 게임 모드 선택, 옵션, eixt 등 UI 제작
- CustomScene - Roomlist(입장 가능한 게임 리스트) 구현
- CustomScene - 방 생성 기능 구현
- CustomScene - 방 입장 기능 구현
- CustomScene - 방 검색 기능 구현
<br><br><br>

## 이성훈 개발 내용
- GameScene - Addressable을 활용하여 최적화 작업 (Photon의 Resource환경 때문에 사용하지않게됨)
- GameScene - 3인칭 카메라 구현
- GameScene - 플레이어의 RagDoll Movement 구현 및 Ragdoll과 관련된 물리학에 따른 리깅 및 구현 (기술스택 부족으로 인한 기획 변경)
- 기획 변경 이후
- GameScene - 플레이어의 움직임 구현 (InputManager, EventManager를 활용하여 모듈화 작업)
- GameScene - 1인칭 카메라 구현
- GameScene - 플레이어의 유닛의 Inverse Kinemachine (IK)를 활용한 Animation Rigging
- GameScene - 플레이어 로컬환경(Photon.IsMine)의 유닛만 조작 가능 및 유저들간의 Position, Rotation 동기화


## 기획서 및 개발 과정
[Notion Link](https://hypnotic-ocelot-c39.notion.site/Project-Solvive-109285de75ba80e5aa81f923a9f34aa1?pvs=4)
<br><br>
![image](https://github.com/user-attachments/assets/ccc4f858-ec25-414d-b2c8-be96b4632580)

