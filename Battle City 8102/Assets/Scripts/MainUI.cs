﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using DG.Tweening;

/***
 * 房主名长限制在13数字大小内
 * 限制来源于RoomInfoFrame中hostNameTextField的长度
 */

public class MainUI : MonoBehaviour {

    public static int screenX = 1280;
    public static int screenY = 720;

    //对应FGUI中的Main,SelectModeComponent,SelectTankComponent
    private GComponent mainComponent;
    private GComponent selectTankComponent;
    private GComponent selectModeComponent;
    private GComponent roomInfoComponent;

    //设置窗口
    private ConfigWindow configWindow;
    private RoomInfoWindow roomInfoWindow;

    //设置列表
    GList roomList;

    //经验条
    public GProgressBar experienceBar;
    //金钱
    public GTextField moneyTextField;
    //房名滚动动效
    private GTweener tweener;


    //测试经验条和金币
    public int money = 100;
    public int experience = 10;
    //测试房间信息
    public List<Room> rooms;

    // Use this for initialization
    void Start () {
        //房间显示测试
        AddRoom();

        //赋值组件
        mainComponent = GetComponent<UIPanel>().ui;
        selectTankComponent = mainComponent.GetChild("selectTankComponent").asCom;
        selectModeComponent = mainComponent.GetChild("selectModeComponent").asCom;
        roomInfoComponent = UIPackage.CreateObject("BattleCity8102","RoomList_Component").asCom;
        //设置钱
        moneyTextField = mainComponent.GetChild("moneyTextField").asTextField;
        moneyTextField.text = money.ToString();
        //设置经验
        experienceBar = mainComponent.GetChild("experienceBar").asProgress;
        experienceBar.value = experience;

        //给按钮注册监听
        //设置按钮按下，显示设置窗口
        configWindow = new ConfigWindow(50);//初始音量设置为50
        mainComponent.GetChild("configButton").asButton.onClick.Add(() => {
            configWindow.Show();
        });
        //对战按钮按下，加载RoomConponent
        selectModeComponent.GetChild("LANBattleButton").onClick.Add(()=> {
            BattleButtonOnClick(roomInfoComponent);
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //加载房间列表
    private void BattleButtonOnClick(GComponent targetComponent)
    {
        //设置组件位置
        targetComponent.SetXY(screenX / 2, 0);
        //移除模式选择组件，添加房间列表组件
        GRoot.inst.RemoveChild(selectModeComponent);
        GRoot.inst.AddChild(targetComponent);
        //注册监听房间按钮，显示房间信息
        roomList = targetComponent.GetChild("roomList").asList;
        roomList.SetVirtual();
        roomList.itemRenderer = RenderListItem;
        //设置房间数目
        roomList.numItems = rooms.Count;
    }

    //渲染roomList
    private void RenderListItem(int index,GObject obj)
    {
        GButton roomButton = obj.asButton;
        roomButton.title = rooms[index].roomName;
        GTextField textField = roomButton.GetChild("title").asTextField;
        //点击房间，显示详细信息
        roomButton.onClick.Add(()=> {
            if (roomInfoWindow != null)
            {
                roomInfoWindow.Dispose();
            }
            //窗口平行显示在roomButton左侧,需要list位置和scrollpane的滚动比例
            roomInfoWindow = new RoomInfoWindow(rooms[index],roomList.x,roomList.y,roomList.height,roomList.scrollPane.contentHeight,roomButton.y,roomList.scrollPane.percY);
            //Debug.Log("percY:"+roomList.scrollPane.percY);
            roomInfoWindow.Show();
            //文本域超出显示范围，滚动
            if (textField.width > roomButton.width)
            {//调整文本域的位置，使文本域向左移动
                tweener = textField.TweenMove(new Vector2(roomList.width - textField.width, 0), 2.0f).OnComplete(() => {
                    //播放完成后清空tweener，重置位置
                    tweener = null;
                    textField.SetXY(0, 0);
                });
            }
        });
    
    }

    //测试房间列表显示
    private void AddRoom()
    {
        int roomNum = 20;
        for (int i = 0; i < roomNum; i++)
        {
            rooms.Add(new Room("求求你们了，相信你们的主播，我tm是真的没有开挂！","卢本伟"+i+"号","绝地海岛","TPP"));
        }
    }

}