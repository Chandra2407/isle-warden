using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OutlineSelectorScript : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;

    private RaycastHit currentRaycastHit;
    private bool hasValidHit;

    public float outlineWidth = 2f;
    private string selectableTag = "Selectables";

    private void Update()
    {
        hasValidHit = false;
        currentRaycastHit = default;

        if (Mouse.current == null)
            return;

        // Remove previous highlight
        if (highlight != null)
        {
            if (highlight.TryGetComponent(out Outline outline))
                outline.enabled = false;

            highlight = null;
        }

        // Ignore mouse over UI
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.CompareTag(selectableTag))
            {
                hasValidHit = true;
                currentRaycastHit = hit;

                if (hit.transform != selection)
                {
                    highlight = hit.transform;

                    if (!highlight.TryGetComponent(out Outline outline))
                    {
                        outline = highlight.gameObject.AddComponent<Outline>();
                        outline.OutlineColor = Color.red;
                        outline.OutlineMode = Outline.Mode.OutlineAll;
                        outline.OutlineWidth = outlineWidth;
                    }

                    outline.enabled = true;
                }
            }
        }

        // Selection
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (highlight != null)
            {
                if (selection != null &&
                    selection.TryGetComponent(out Outline oldOutline))
                {
                    oldOutline.enabled = false;
                }

                selection = highlight;

                if (!selection.TryGetComponent(out Outline outline))
                {
                    outline = selection.gameObject.AddComponent<Outline>();
                    outline.OutlineColor = Color.red;
                    outline.OutlineMode = Outline.Mode.OutlineAll;
                    outline.OutlineWidth = outlineWidth;
                }

                outline.enabled = true;
                highlight = null;
            }
            else
            {
                if (selection != null &&
                    selection.TryGetComponent(out Outline outline))
                {
                    outline.enabled = false;
                }

                selection = null;
            }
        }
    }

    public bool TryGetHoveredSelectable(out RaycastHit hit)
    {
        hit = currentRaycastHit;
        return hasValidHit;
    }

    public Transform SelectedObject => selection;
}