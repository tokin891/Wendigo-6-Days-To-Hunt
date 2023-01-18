using System.Collections;
using UnityEngine.Events;
using UnityEngine;

namespace Menu.Logo
{
    public class IntroLogo : MonoBehaviour
    {
        //----------Inspector Details
        [Header("Type")]
        [SerializeField] TypeLogoIntro _SetLogo;
        [SerializeField] bool loginGamejolt;
        [Header("Proportions")]
        [SerializeField,
            Tooltip("Seconds to end animation if Type is 'Animation'")] float TimeToEnd;
        [Header("Next Logo")]
        [SerializeField] IntroLogo IL;
        [Header("Events"), Space]
        [SerializeField] UnityEvent _OnStart;
        [SerializeField] UnityEvent _OnEnd;

        private void Awake()
        {
            if (_SetLogo == TypeLogoIntro.Animation)
            { GetComponent<Animator>().SetTrigger("Play"); }
            if (_SetLogo == TypeLogoIntro.Seconds)
            { StartCoroutine(CountToEnd()); }

            _OnStart.Invoke();
        }
        private void Update()
        {
            if(_SetLogo == TypeLogoIntro.Click && Input.GetMouseButtonDown(0))
            { EndAnimation(); }
            if (loginGamejolt)
                EndAnimation();
        }

        #region Methods

        public void EndAnimation()
        {
            GetComponent<Animator>().SetTrigger("Exit");
        }
        public void loginGJ()
        {
            GameJolt.UI.GameJoltUI.Instance.ShowSignIn((bool signInSuccess) => {
                Photon.Pun.PhotonNetwork.NickName = GameJolt.API.GameJoltAPI.Instance.CurrentUser.Name;
                IL.loginGamejolt = true;
            });
        }
        public void DestroyThis()
        {
            if (IL != null)
            {
                IL.gameObject.SetActive(true);
            }
            else
                MenuVendigoTrip.instance.CountShowMenu();

            _OnEnd.Invoke();
            Destroy(gameObject);
        }
        private IEnumerator CountToEnd()
        {
            yield return new WaitForSeconds(TimeToEnd);

            EndAnimation();
        }

        #endregion
    }

    public enum TypeLogoIntro
    {
        Animation,
        Click,
        Seconds,
        Void
    }
}
