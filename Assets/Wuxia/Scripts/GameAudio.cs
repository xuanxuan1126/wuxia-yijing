using UnityEngine;
namespace Wuxia {
 public class GameAudio:MonoBehaviour {
  AudioSource[] music,voices;float targetVolume;bool boss,muted,paused;int index;
  public void Initialize(float volume){targetVolume=volume;music=new AudioSource[2];for(int i=0;i<2;i++){var g=new GameObject("古风音乐 "+i);g.transform.SetParent(transform);var a=g.AddComponent<AudioSource>();a.clip=Resources.Load<AudioClip>("Audio/"+(i==0?"musicExplore":"musicMaster"));a.loop=true;a.playOnAwake=false;a.volume=0;var low=g.AddComponent<AudioLowPassFilter>();low.cutoffFrequency=1200;music[i]=a;}voices=new AudioSource[24];for(int i=0;i<voices.Length;i++){var g=new GameObject("音效声部 "+i);g.transform.SetParent(transform);voices[i]=g.AddComponent<AudioSource>();voices[i].playOnAwake=false;}}
  public void StartMusic(){if(!Application.isPlaying)return;foreach(var a in music)if(!a.isPlaying)a.Play();paused=false;}
  public void Boss(bool value)=>boss=value;
  public void ToggleMute(){muted=!muted;foreach(var a in music)a.mute=muted;foreach(var a in voices)a.mute=muted;}
  public void Pause(bool value){paused=value;foreach(var a in music){if(value)a.Pause();else a.UnPause();}foreach(var a in voices){if(value)a.Pause();else a.UnPause();}}
  public void Stop(){foreach(var a in music)a.Stop();foreach(var a in voices)a.Stop();}
  public void Play(string kind,float volume=.28f){if(!Application.isPlaying||paused||muted)return;var clip=Resources.Load<AudioClip>("Audio/"+kind);if(!clip)return;for(int j=0;j<voices.Length;j++){int n=(index+j)%voices.Length;if(voices[n].isPlaying)continue;var a=voices[n];a.clip=clip;a.volume=volume;a.pitch=Random.Range(.96f,1.04f);a.Play();index=(n+1)%voices.Length;return;}}
  void Update(){if(music==null||paused)return;for(int i=0;i<2;i++)music[i].volume=Mathf.MoveTowards(music[i].volume,(boss?(i==1):(i==0))?targetVolume:0,Time.unscaledDeltaTime*targetVolume/3.5f);}
 }
}
