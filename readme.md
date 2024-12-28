# Kimchi-Run AI


<img src="/static/Kimchi-run-normal.gif" width="820" height="480">
<div align="center"><b>Normal</b></div>
<img src="/static/Kimchi-run-debug.png" width="820" height="480">
<div align="center"><b>Debug Features</b></div>


<div align="center">

**[Unity ML-Agents ToolKit](https://unity-technologies.github.io/ml-agents/) Kimchi-Run AI Sample-Play**

</div>

---

## 프로젝트 소개
Unity ML Agent를 사용한 Kimchi-Run 강화학습 프로젝트입니다. 기본적인 플레이는 [노마드 코더 김장 게임](https://www.youtube.com/watch?v=A58_FWqiekI)을 따릅니다. 거기에 AI Agent 학습을 시켜 AI와 플레이어 간 대결을 할 수 있도록 지원하였습니다.  

- **개발 기간 (1주)**

## 주요 기능
- ML Agent로 플레이어 AI를 학습
- ML Agent가 지원하는 모든 기능을 지원
- 강화학습 알고리즘 적용 AI Agent 대결 기능 추가
- 학습 환경 prefabs를 베이스로한 MultiArea Learning 가능
- F1 Key를 통해 사전설정된 Debug 확인가능

## 설치 환경
1. Unity 6000.0.31f1 (Latest!)
2. ML Agents package 3.0.0 

## 실행 방법
- Unity Editor에서 열기
- PlayWithAI Scene을 비활성화 하고
- Training Scene을 활성화 시켜 Python API로 학습 진행

- Example Code
`mlagents-learn ./Config/ppo/b-transfer.yaml --run-id=kimchirun_f --force --time-scale=5`
더욱 빠른 배속을 원하시면 `--time-scale` 옵션을 ~20까지 올리면 됩니다.
기본 속도로 학습을 원하시면 `--time-scale=1 --capture-frame-rate=0--target-frame-rate=-1` 옵션을 주면 됩니다.

## 학습 알고리즘
- Proximal Policy Optimization (PPO)

## Experiments

가장 Rewards가 높은 모델이 포함되어있으나, 실험했던 Cumulaitve Reward Scalar를 공개

<img src="/static/cum_reward.png">


<b style="color:green">Kimchirun_e</b>에 해당하는 모델을 3.75M까지 훈련한 후, 설정을 바꾸어 <b style="color:purple">Kimchirun_f</b> 모델에 Transfer Learning으로 5M 더 진행했습니다.

## Observation, Action, Reward


**Observation**
- 게임 속도 8~20값에 빠를수록 커지는 0-1 Norm을 적용시켜 측정했습니다. 
- 플레이어로부터 가장 가까운적의 거리를 가까울 수록 커지는 0-1 Norm 적용시켜 측정했습니다.

**Action**
- Player Agent가 점프 할 수 있는 경우 (땅에 있는 경우) 에만 Obs 측정 및 Action 하도록 하였습니다.
- 0 = No_Action: 아무런 액션도 하지않습니다.
- 1 = Jump: 점프합니다.
- Action 사이의 Delay는 없습니다

**Reward**
learning_rate를 constant로하고 Reward를 학습 수시로 변경했습니다.

- Kimchi-Run_e
  - Action시 reward 0.001 -> 0.01 -> 0.03
  - 장애물 회피시 +1, 맞을시 -1점
    - **특정 Step에서 변경**
  - Action시 reward 0.01 점프 할시 -0.1 
  - 장애물 회피시 +1, 맞을시 -1점
    - **특정 step에서 변경**
  - No_Action시 0.05 -> 0.01
  - 장애물 회피시 +1, 맞을시 -3점
- Kimchi-Run_e
  - Kimchi-Run_e의 3.75M step의 .pt 모델파일을 5M 추가로 학습한 모델
  - **지금 게임에는 이 모델이 기본값으로 적용 되어 있습니다.**
  



